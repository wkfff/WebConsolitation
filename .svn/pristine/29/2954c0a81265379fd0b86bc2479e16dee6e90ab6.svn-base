using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.PfhdService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD
{
    public class PfhdController : SchemeBoundController
    {
        private readonly IAuthService auth;
        private readonly IChangeLogService logService;
        private readonly IPfhdService pfhdService;

        public PfhdController()
        {
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
            pfhdService = Resolver.Get<IPfhdService>();
        }

        /* Отдельный метод, обернутый в транзакцию, нужен по причине того,
         * что в документе еще могут быть не заведены записи
         * и они будут созданы автоматически,
         * и соответственно требовать записи в БД.
         */
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Read(int? parentId)
        {
            try
            {
                return new RestResult
                    {
                        Success = true, 
                        Data = pfhdService.GetItems(parentId)
                            .Select(
                                item => new PfhdViewModel
                                    {
                                        RefParameterID = item.RefParametr.ID, 
                                        ID = item.ID, 
                                        TotnonfinAssets = item.totnonfinAssets, 
                                        RealAssets = item.realAssets, 
                                        HighValPersAssets = item.highValPersAssets, 
                                        FinAssets = item.finAssets, 
                                        Income = item.income, 
                                        Expense = item.expense, 
                                        FinCircum = item.finCircum, 
                                        KreditExpir = item.kreditExpir, 
                                        StateTaskGrant = item.stateTaskGrant, 
                                        ActionGrant = item.actionGrant, 
                                        BudgetaryFunds = item.budgetaryFunds, 
                                        PaidServices = item.paidServices, 
                                        PlanPayments = item.planPayments, 
                                        PlanInpayments = item.planInpayments, 
                                        LabourRemuneration = item.labourRemuneration, 
                                        TelephoneServices = item.telephoneServices, 
                                        FreightServices = item.freightServices, 
                                        PublicServeces = item.publicServeces, 
                                        Rental = item.rental, 
                                        MaintenanceCosts = item.maintenanceCosts, 
                                        MainFunds = item.mainFunds, 
                                        FictitiousAssets = item.fictitiousAssets, 
                                        TangibleAssets = item.tangibleAssets, 
                                        Publish = item.publish, 
                                        NumberStr = item.NumberStr, 
                                        Plan3Year = Convert.ToInt32(item.RefParametr.PlanThreeYear), 
                                        RefYearFormID = item.RefParametr.RefYearForm.ID
                                    })
                    };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "Ошибка загрузки Плана ФХД: " + e.Message, Data = null };
            }
        }
        
        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Save(string data, int parentId)
        {
            try
            {
                F_Fin_finActPlan newPfhd = JavaScriptDomainConverter<F_Fin_finActPlan>.DeserializeSingle(data);
                newPfhd.RefParametr = pfhdService.GetItem<F_F_ParameterDoc>(parentId);

                foreach (var info in newPfhd.GetType().GetProperties().Where(p => p.PropertyType == typeof(decimal?) && p.GetValue(newPfhd, null) == null))
                {
                    info.SetValue(newPfhd, decimal.Zero, null);
                }

                string msg = "Запись обновлена";
                if (newPfhd.ID < 0)
                {
                    newPfhd.ID = 0;
                    msg = "Новая запись добавлена";
                }

                var validationError = ValidateData(newPfhd);
                if (validationError == string.Empty)
                {
                    pfhdService.Save(newPfhd);
                    logService.WriteChangeDocDetail(newPfhd.RefParametr);
                    return new RestResult
                        {
                            Success = true, 
                            Message = msg, 
                            Data = pfhdService.GetItems<F_Fin_finActPlan>()
                                .Where(v => v.ID == newPfhd.ID)
                                .Select(
                                    item => new PfhdViewModel
                                        {
                                            RefParameterID = item.RefParametr.ID, 
                                            ID = item.ID, 
                                            TotnonfinAssets = item.totnonfinAssets, 
                                            RealAssets = item.realAssets, 
                                            HighValPersAssets = item.highValPersAssets, 
                                            FinAssets = item.finAssets, 
                                            Income = item.income, 
                                            Expense = item.expense, 
                                            FinCircum = item.finCircum, 
                                            KreditExpir = item.kreditExpir, 
                                            StateTaskGrant = item.stateTaskGrant, 
                                            ActionGrant = item.actionGrant, 
                                            BudgetaryFunds = item.budgetaryFunds, 
                                            PaidServices = item.paidServices, 
                                            PlanPayments = item.planPayments, 
                                            PlanInpayments = item.planInpayments, 
                                            LabourRemuneration = item.labourRemuneration, 
                                            TelephoneServices = item.telephoneServices, 
                                            FreightServices = item.freightServices, 
                                            PublicServeces = item.publicServeces, 
                                            Rental = item.rental, 
                                            MaintenanceCosts = item.maintenanceCosts, 
                                            MainFunds = item.mainFunds, 
                                            FictitiousAssets = item.fictitiousAssets, 
                                            TangibleAssets = item.tangibleAssets, 
                                            Publish = item.publish, 
                                            NumberStr = item.NumberStr, 
                                            Plan3Year = Convert.ToInt32(item.RefParametr.PlanThreeYear), 
                                            RefYearFormID = item.RefParametr.RefYearForm.ID
                                        })
                        };
                }

                throw new InvalidDataException(validationError);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "Ошибка создания Плана ФХД: " + e.Message, Data = null };
            }
        }
        
        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckCloseDocs(recId))
            {
                return new RestResult
                    {
                        Success = false, 
                        Message = "Нет закрытых документов"
                    };
            }

            var pfhds = pfhdService.GetItems<F_Fin_finActPlan>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();

            var pfhdsIsEmpty = true;

            foreach (var t in
                pfhds.Where(
                    t =>
                    !(t.totnonfinAssets == 0 && t.realAssets == 0 && t.highValPersAssets == 0 && t.finAssets == 0 && t.income == 0 && t.expense == 0 && t.finCircum == 0 && t.kreditExpir == 0
                      && t.planInpayments == 0 && t.stateTaskGrant == 0 && t.actionGrant == 0 && t.budgetaryFunds == 0 && t.paidServices == 0 && t.planPayments == 0
                      && t.labourRemuneration == 0 && t.telephoneServices == 0 && t.freightServices == 0 && t.publicServeces == 0 && t.rental == 0 && t.maintenanceCosts == 0
                      && t.mainFunds == 0 && t.fictitiousAssets == 0 && t.tangibleAssets == 0 && t.publish == 0)))
            {
                pfhdsIsEmpty = false;
            }

            if (pfhdsIsEmpty)
            {
                return new RestResult
                    {
                        Success = true, 
                        Message = "Документ пуст"
                    };
            }

            return new RestResult
                {
                    Success = false, 
                    Message = "Документ не пуст"
                };
        }

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            F_F_ParameterDoc formData = pfhdService.GetItems<F_F_ParameterDoc>().First(x => x.ID == recId);
            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;
            try
            {
                var pfhds = pfhdService.GetItems<F_Fin_finActPlan>()
                    .Where(
                        x =>
                        x.RefParametr.ID == recId).ToList();

                var finActPlans = pfhdService.GetItems<F_Fin_finActPlan>()
                    .Where(
                        x =>
                        x.RefParametr.ID == idOfLastDoc).ToList();

                if (finActPlans.Count > 0)
                {
                    // foreach (var fFinFinActPlan in finActPlans)
                    for (var i = 0; i < finActPlans.Count; i++)
                    {
                        pfhds[i].totnonfinAssets = finActPlans[i].totnonfinAssets;
                        pfhds[i].realAssets = finActPlans[i].realAssets;
                        pfhds[i].highValPersAssets = finActPlans[i].highValPersAssets;
                        pfhds[i].finAssets = finActPlans[i].finAssets;
                        pfhds[i].income = finActPlans[i].income;
                        pfhds[i].expense = finActPlans[i].expense;
                        pfhds[i].finCircum = finActPlans[i].finCircum;
                        pfhds[i].kreditExpir = finActPlans[i].kreditExpir;
                        pfhds[i].planInpayments = finActPlans[i].planInpayments;
                        pfhds[i].stateTaskGrant = finActPlans[i].stateTaskGrant;
                        pfhds[i].actionGrant = finActPlans[i].actionGrant;
                        pfhds[i].budgetaryFunds = finActPlans[i].budgetaryFunds;
                        pfhds[i].paidServices = finActPlans[i].paidServices;
                        pfhds[i].planPayments = finActPlans[i].planPayments;
                        pfhds[i].labourRemuneration = finActPlans[i].labourRemuneration;
                        pfhds[i].telephoneServices = finActPlans[i].telephoneServices;
                        pfhds[i].freightServices = finActPlans[i].freightServices;
                        pfhds[i].publicServeces = finActPlans[i].publicServeces;
                        pfhds[i].rental = finActPlans[i].rental;
                        pfhds[i].maintenanceCosts = finActPlans[i].maintenanceCosts;
                        pfhds[i].mainFunds = finActPlans[i].mainFunds;
                        pfhds[i].fictitiousAssets = finActPlans[i].fictitiousAssets;
                        pfhds[i].tangibleAssets = finActPlans[i].tangibleAssets;
                        pfhds[i].publish = finActPlans[i].publish;
                        pfhds[i].NumberStr = finActPlans[i].NumberStr;

                        pfhdService.Save(pfhds[i]);
                    }
                }

                pfhdService.Save(formData);
                pfhdService.CommitChanges();

                var capFundsRepository = pfhdService.GetRepository<F_Fin_CapFunds>();
                var capFunds = capFundsRepository.FindAll()
                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (capFunds.Count > 0)
                {
                    foreach (var finCapFunds in capFunds)
                    {
                        var capFund = new F_Fin_CapFunds
                            {
                                SourceID = finCapFunds.SourceID, 
                                TaskID = finCapFunds.TaskID, 
                                Name = finCapFunds.Name, 
                                funds = finCapFunds.funds, 
                                RefParametr = formData
                            };
                        capFundsRepository.Save(capFund);
                    }
                }

                var realAssetsRepository = pfhdService.GetRepository<F_Fin_realAssFunds>();
                var realAssets = realAssetsRepository.FindAll()
                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (realAssets.Count > 0)
                {
                    foreach (var finRealAssFunds in realAssets)
                    {
                        var realAsset = new F_Fin_realAssFunds
                            {
                                SourceID = finRealAssFunds.SourceID, 
                                TaskID = finRealAssFunds.TaskID, 
                                Name = finRealAssFunds.Name, 
                                funds = finRealAssFunds.funds, 
                                RefParametr = formData
                            };

                        realAssetsRepository.Save(realAsset);
                    }
                }

                var otherGrantRepository = pfhdService.GetRepository<F_Fin_othGrantFunds>();
                var otherGrants = otherGrantRepository.FindAll()
                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (otherGrants.Count > 0)
                {
                    foreach (var finOthGrantFunds in otherGrants)
                    {
                        var otherGrant = new F_Fin_othGrantFunds
                            {
                                SourceID = finOthGrantFunds.SourceID, 
                                TaskID = finOthGrantFunds.TaskID, 
                                RefOtherGrant = finOthGrantFunds.RefOtherGrant, 
                                KOSGY = finOthGrantFunds.KOSGY, 
                                funds = finOthGrantFunds.funds, 
                                RefParametr = formData
                            };

                        otherGrantRepository.Save(otherGrant);
                    }
                }

                var finothGrantFundsRepository = pfhdService.GetRepository<F_Fin_othGrantFunds>();

                var finFinActPlanRepository = pfhdService.GetRepository<F_Fin_finActPlan>();

                var val = finothGrantFundsRepository.FindAll().Where(x => x.RefParametr.ID == recId).Sum(x => x.funds) == null
                              ? 0
                              : finothGrantFundsRepository.FindAll().Where(x => x.RefParametr.ID == recId).Sum(x => x.funds);

                var pfhd = finFinActPlanRepository.FindAll().First(x => (x.RefParametr.ID == recId) && (x.NumberStr == 0));

                if (pfhd.actionGrant != val)
                {
                    pfhd.actionGrant = val;

                    finFinActPlanRepository.Save(pfhd);
                }

                logService.WriteChangeDocDetail(formData);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Данные скопированы"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult ExportToXml(int recId)
        {
            using (var zipStream = new MemoryStream())
            {
                using (Package package = Package.Open(zipStream, FileMode.CreateNew))
                {
                    var documentName = "financialActivityPlan";
                    var documentContent = ExportFinancialActivityPlanService.Serialize(auth, pfhdService.Load<F_F_ParameterDoc>(recId));

                    Uri partUriFinancialActivityPlan = PackUriHelper.CreatePartUri(
                        new Uri(string.Format("{0}.xml", documentName), UriKind.Relative));
                    PackagePart packagePartFinancialActivityPlan = package.CreatePart(
                        partUriFinancialActivityPlan, 
                        MediaTypeNames.Text.Xml, 
                        CompressionOption.Maximum);
                    using (var streamWriter = new BinaryWriter(packagePartFinancialActivityPlan.GetStream()))
                    {
                        streamWriter.Write(documentContent);
                    }

                    documentName = "actionGrant";
                    documentContent = ExportActionGrantService.Serialize(pfhdService.Load<F_F_ParameterDoc>(recId));

                    Uri partUriActionGrant = PackUriHelper.CreatePartUri(
                        new Uri(string.Format("{0}.xml", documentName), UriKind.Relative));
                    PackagePart packagePartActionGrant = package.CreatePart(
                        partUriActionGrant, 
                        MediaTypeNames.Text.Xml, 
                        CompressionOption.Maximum);
                    using (var streamWriter = new BinaryWriter(packagePartActionGrant.GetStream()))
                    {
                        streamWriter.Write(documentContent);
                    }
                }

                return File(
                    zipStream.ToArray(), 
                    MediaTypeNames.Application.Zip, 
                    "PFHD" + DateTime.Now.ToString("yyyymmddhhmmss") + ".zip");
            }
        }

        protected string ValidateData(F_Fin_finActPlan pfhd)
        {
            var res = string.Empty;

            if (pfhd.totnonfinAssets < (pfhd.realAssets + pfhd.highValPersAssets))
            {
                res = ((res == string.Empty) ? "<br />" : (res + ",<br /><br />"))
                      + "значение 'Общая сумма балансовой стоимости нефинансовых активов' должно быть не меньше суммы значений"
                      + " 'Недвижимое имущество' и "
                      + "'Особо ценное движимое имущество'";
            }

            if (pfhd.planInpayments < pfhd.stateTaskGrant + pfhd.actionGrant + pfhd.budgetaryFunds + pfhd.paidServices)
            {
                res = (res == string.Empty ? "<br />" : (res + ",<br /><br />"))
                      + "значение 'Планируемая сумма поступлений, всего' должно быть больше суммы значений "
                      + "'Субсидии на выполнение задания', "
                      + "'Целевые субсидии', "
                      + "'Бюджетные инвестиции' и "
                      + "'Оказание платных услуг (выполнения работ) и приносящая доход деятельность'";
            }

            if (pfhd.planPayments < (pfhd.labourRemuneration + pfhd.telephoneServices + pfhd.freightServices
                                     + pfhd.publicServeces + pfhd.rental + pfhd.maintenanceCosts
                                     + pfhd.mainFunds + pfhd.fictitiousAssets + pfhd.tangibleAssets))
            {
                res = (res == string.Empty ? "<br />" : (res + ",<br /><br />"))
                      + "значение 'Планируемая сумма выплат, всего' должно быть больше суммы значений "
                      + "'Оплата труда и начисления на выплаты по оплате труда', "
                      + "'Оплата услуг связи', "
                      + "'Оплата транспортных услуг', "
                      + "'Оплата коммунальных услуг', "
                      + "'Арендная плата за пользование имуществом', "
                      + "'Оплата услуг по содержанию имущества', "
                      + "'Приобретение основных средств', "
                      + "'Приобретение нематериальных активов' и "
                      + "'Приобретение материальных запасов'";
            }

            return res;
        }
    }
}
