using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.PfhdService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD
{
    public class Pfhd2017ViewController : SchemeBoundController
    {
        private readonly IChangeLogService logService;
        private readonly IPfhd2017Service pfhd2017Service;

        private readonly FinancialIndexViewModel financialIndexViewModel = new FinancialIndexViewModel();
        private readonly TemporaryResourcesViewModel temporaryResourcesViewModel = new TemporaryResourcesViewModel();
        private readonly ReferenceViewModel referenceViewModel = new ReferenceViewModel();

        public Pfhd2017ViewController()
        {
            logService = Resolver.Get<IChangeLogService>();
            pfhd2017Service = Resolver.Get<IPfhd2017Service>();
        }
        
        public ActionResult FinancialIndexFormRead(int docId)
        {
            var data = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).FinancialIndex
                .Select(p => (FinancialIndexViewModel)financialIndexViewModel.GetModelByDomain(p));

            return new RestResult { Data = data, Success = true };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult FinancialIndexFormSave(FormCollection values, int docId)
        {
            var result = new AjaxFormResult();

            try
            {
                var model = new FinancialIndexViewModel
                {
                    MoneyInstitutions = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.MoneyInstitutions)], (decimal?)null),
                    FundsAccountsInstitution = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FundsAccountsInstitution)], (decimal?)null),
                    FundsPlacedOnDeposits = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FundsPlacedOnDeposits)], (decimal?)null)
                };
                var validationError = model.ValidateData(docId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).FinancialIndex.FirstOrDefault() 
                                            ?? new F_F_FinancialIndex
                                            {
                                                       ID = 0,
                                                       RefParametr = pfhd2017Service.Load<F_F_ParameterDoc>(docId)
                                                   };

                // todo наваять конвертер преобразующий FormCollection в Domain JavaScriptDomainConverter<Service2016ViewModel>.DeserializeSingle(data);
                // todo метод расширения GetValue для FormCollection
                record.NonFinancialAssets = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.NonFinancialAssets)], (decimal?)null);
                record.RealAssets = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.RealAssets)], (decimal?)null);
                record.RealAssetsDepreciatedCost = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.RealAssetsDepreciatedCost)], (decimal?)null);
                record.HighValuePersonalAssets = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.HighValuePersonalAssets)], (decimal?)null);
                record.HighValuePADepreciatedCost = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.HighValuePADepreciatedCost)], (decimal?)null);
                record.FinancialAssets = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FinancialAssets)], (decimal?)null);
                record.MoneyInstitutions = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.MoneyInstitutions)], (decimal?)null);
                record.FundsAccountsInstitution = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FundsAccountsInstitution)], (decimal?)null);
                record.FundsPlacedOnDeposits = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FundsPlacedOnDeposits)], (decimal?)null);
                record.OtherFinancialInstruments = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.OtherFinancialInstruments)], (decimal?)null);
                record.DebitIncome = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.DebitIncome)], (decimal?)null);
                record.DebitExpense = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.DebitExpense)], (decimal?)null);
                record.FinancialCircumstanc = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.FinancialCircumstanc)], (decimal?)null);
                record.Debentures = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.Debentures)], (decimal?)null);
                record.AccountsPayable = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.AccountsPayable)], (decimal?)null);
                record.KreditExpired = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => financialIndexViewModel.KreditExpired)], (decimal?)null);

                pfhd2017Service.Save(record);
                logService.WriteChangeDocDetail(pfhd2017Service.GetItem<F_F_ParameterDoc>(docId));

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        public ActionResult TemporaryResourcesFormRead(int docId)
        {
            var data = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).TemporaryResources
                .Select(p => new TemporaryResourcesViewModel
                {
                    BalanceBeginningYear = p.BalanceBeginningYear,
                    BalanceEndYear = p.BalanceEndYear,
                    Income = p.Income,
                    Disposals = p.Disposals
                });

            return new RestResult { Data = data, Success = true };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult TemporaryResourcesFormSave(FormCollection values, int docId)
        {
            var result = new AjaxFormResult();

            try
            {
                var record = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).TemporaryResources.FirstOrDefault()
                                            ?? new F_F_TemporaryResources
                                            {
                                                ID = 0,
                                                RefParametr = pfhd2017Service.Load<F_F_ParameterDoc>(docId)
                                            };

                // todo наваять конвертер преобразующий FormCollection в Domain
                record.BalanceBeginningYear = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => temporaryResourcesViewModel.BalanceBeginningYear)], (decimal?)null);
                record.BalanceEndYear = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => temporaryResourcesViewModel.BalanceEndYear)], (decimal?)null);
                record.Income = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => temporaryResourcesViewModel.Income)], (decimal?)null);
                record.Disposals = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => temporaryResourcesViewModel.Disposals)], (decimal?)null);
                
                pfhd2017Service.Save(record);
                logService.WriteChangeDocDetail(pfhd2017Service.GetItem<F_F_ParameterDoc>(docId));

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        public ActionResult ReferenceFormRead(int docId)
        {
            var data = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).Reference
                .Select(p => new ReferenceViewModel
                {
                    AmountPublicLiabilities = p.AmountPublicLiabilities,
                    VolumeBudgetIinvestments = p.VolumeBudgetIinvestments,
                    AmountTemporaryOrder = p.AmountTemporaryOrder
                });

            return new RestResult { Data = data, Success = true };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ReferenceFormSave(FormCollection values, int docId)
        {
            var result = new AjaxFormResult();

            try
            {
                var record = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).Reference.FirstOrDefault()
                                            ?? new F_F_Reference
                                            {
                                                ID = 0,
                                                RefParametr = pfhd2017Service.Load<F_F_ParameterDoc>(docId)
                                            };

                // todo наваять конвертер преобразующий FormCollection в Domain
                record.AmountPublicLiabilities = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => referenceViewModel.AmountPublicLiabilities)], (decimal?)null);
                record.VolumeBudgetIinvestments = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => referenceViewModel.VolumeBudgetIinvestments)], (decimal?)null);
                record.AmountTemporaryOrder = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => referenceViewModel.AmountTemporaryOrder)], (decimal?)null);
                
                pfhd2017Service.Save(record);
                logService.WriteChangeDocDetail(pfhd2017Service.GetItem<F_F_ParameterDoc>(docId));

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        #region ExpensePaymentIndex
        
        public RestResult ExpensePaymentGridRead(int docId)
        {
            ExpensePaymentContent(docId);

            var data = pfhd2017Service.GetItems<F_Fin_ExpensePaymentIndex>()
                        .Where(x => x.RefParametr.ID.Equals(docId))
                        .Select(
                            p =>
                            new ExpensePaymentIndexViewModel
                            {
                                ID = p.ID,
                                Name = p.Name,
                                LineCode = p.LineCode,
                                Year = p.Year,
                                TotalSumNextYear = p.TotalSumNextYear,
                                TotalSumFirstPlanYear = p.TotalSumFirstPlanYear,
                                TotalSumSecondPlanYear = p.TotalSumSecondPlanYear,
                                Fz44SumNextYear = p.Fz44SumNextYear,
                                Fz44SumFirstPlanYear = p.Fz44SumFirstPlanYear,
                                Fz44SumSecondPlanYear = p.Fz44SumSecondPlanYear,
                                Fz223SumNextYear = p.Fz223SumNextYear,
                                Fz223SumFirstPlanYear = p.Fz223SumFirstPlanYear,
                                Fz223SumSecondPlanYear = p.Fz223SumSecondPlanYear
                            });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult ExpensePaymentGridSave(string data, int docId)
        {
            try
            {
                var model = JavaScriptDomainConverter<ExpensePaymentIndexViewModel>.DeserializeSingle(data);
                var validationError = model.ValidateData(docId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_Fin_ExpensePaymentIndex>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId);

                pfhd2017Service.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = pfhd2017Service.GetItems<F_Fin_ExpensePaymentIndex>()
                                .Where(p => p.ID == record.ID)
                                .Select(
                                    p => new ExpensePaymentIndexViewModel
                                    {
                                        ID = p.ID,
                                        Name = p.Name,
                                        LineCode = p.LineCode,
                                        Year = p.Year,
                                        TotalSumNextYear = p.TotalSumNextYear,
                                        TotalSumFirstPlanYear = p.TotalSumFirstPlanYear,
                                        TotalSumSecondPlanYear = p.TotalSumSecondPlanYear,
                                        Fz44SumNextYear = p.Fz44SumNextYear,
                                        Fz44SumFirstPlanYear = p.Fz44SumFirstPlanYear,
                                        Fz44SumSecondPlanYear = p.Fz44SumSecondPlanYear,
                                        Fz223SumNextYear = p.Fz223SumNextYear,
                                        Fz223SumFirstPlanYear = p.Fz223SumFirstPlanYear,
                                        Fz223SumSecondPlanYear = p.Fz223SumSecondPlanYear
                                    })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult ExpensePaymentGridDelete(int id, int docId)
        {
            return pfhd2017Service.DeleteDocDetailAction<F_Fin_ExpensePaymentIndex>(id, docId);
        }

        [Transaction]
        public ActionResult ExpensePaymentGridCalculateSumm(int docId)
        {
            try
            {
                var rows = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).ExpensePaymentIndex.ToList();
                
                ExpensePaymentGridSummRow(rows.FirstOrDefault(x => x.LineCode.Equals("1001")));

                ExpensePaymentGridSummRow(rows.FirstOrDefault(x => x.LineCode.Equals("2001")));

                ExpensePaymentGridSummRows(new[] { "1001", "2001" }, "0001", rows);
                
                return new RestResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new RestResult
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }
        
        #endregion

        #region PlanPaymentIndex

        public RestResult PlanPaymentIndexGridRead(int docId, int period)
        {
            PlanPaymentIndexContent(docId, period);
            
            var data = pfhd2017Service.GetItems<F_F_PlanPaymentIndex>()
                .Where(x => x.RefParametr.ID.Equals(docId) && x.Period.Equals(period))
                .Select(
                    p => new PlanPaymentIndexViewModel
                    {
                        ID = p.ID,
                        Name = p.Name,
                        LineCode = p.LineCode,
                        Kbk = p.Kbk,
                        Total = p.Total,
                        FinancialProvision = p.FinancialProvision,
                        SubsidyFinSupportFfoms = p.SubsidyFinSupportFfoms,
                        SubsidyOtherPurposes = p.SubsidyOtherPurposes,
                        CapitalInvestment = p.CapitalInvestment,
                        HealthInsurance = p.HealthInsurance,
                        ServiceTotal = p.ServiceTotal,
                        ServiceGrant = p.ServiceGrant
                    });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult PlanPaymentIndexGridSave(string data, int docId, int period)
        {
            try
            {
                var model = JavaScriptDomainConverter<PlanPaymentIndexViewModel>.DeserializeSingle(data);
                var validationError = model.ValidateData(docId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_PlanPaymentIndex>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId);
                record.Period = period;

                pfhd2017Service.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = pfhd2017Service.GetItems<F_F_PlanPaymentIndex>()
                                .Where(p => p.ID == record.ID)
                                .Select(
                                    p => new PlanPaymentIndexViewModel
                                    {
                                        ID = p.ID,
                                        Name = p.Name,
                                        LineCode = p.LineCode,
                                        Kbk = p.Kbk,
                                        Total = p.Total,
                                        FinancialProvision = p.FinancialProvision,
                                        SubsidyFinSupportFfoms = p.SubsidyFinSupportFfoms,
                                        SubsidyOtherPurposes = p.SubsidyOtherPurposes,
                                        CapitalInvestment = p.CapitalInvestment,
                                        HealthInsurance = p.HealthInsurance,
                                        ServiceTotal = p.ServiceTotal,
                                        ServiceGrant = p.ServiceGrant
                                    })
            };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult PlanPaymentIndexGridDelete(int id, int docId)
        {
            return pfhd2017Service.DeleteDocDetailAction<F_Fin_ExpensePaymentIndex>(id, docId);
        }

        #endregion

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            try
            {
                pfhd2017Service.CopyContent(recId);
                logService.WriteChangeDocDetail(pfhd2017Service.GetItem<F_F_ParameterDoc>(recId));

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

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckDocs(recId))
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Нет закрытых документов"
                };
            }

            if (pfhd2017Service.CheckDocEmpty(recId))
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

        public ActionResult ExportToXml(int recId)
        {
            using (var zipStream = new MemoryStream())
            {
                using (Package package = Package.Open(zipStream, FileMode.CreateNew))
                {
                    var documentName = "financialActivityPlan2017";
                    var documentContent = ExportFinancialActivityPlan2017Service.Serialize(pfhd2017Service.GetItem<F_F_ParameterDoc>(recId));

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

                    try
                    {
                        documentName = "actionGrant";
                        documentContent = ExportActionGrantService.Serialize(pfhd2017Service.GetItem<F_F_ParameterDoc>(recId));

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
                    catch (InvalidDataException)
                    {
                        // данное исключение возникает если отсутствуют данные по actionGrant, просто не формируем
                    }
                }

                return File(
                    zipStream.ToArray(),
                    MediaTypeNames.Application.Zip,
                    "PFHD2017" + DateTime.Now.ToString("yyyymmddhhmmss") + ".zip");
            }
        }

        [Transaction]
        private void PlanPaymentIndexContent(int docId, int period)
        {
            var docContent = pfhd2017Service.GetItems<T_F_IndicatorsPfhd>()
                    .Where(x => x.Detail.Equals(0)).OrderBy(x => x.Code);

            var docData = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(period))
                        .Select(x => new
                        {
                            x.ID,
                            x.LineCode,
                            x.Name
                        }).ToList();

            foreach (var contentItem in docContent)
            {
                // если нет строчки то добавляем
                if (!docData.Any(x => x.LineCode.Equals(contentItem.Code)))
                {
                    var record = new F_F_PlanPaymentIndex
                    {
                        ID = 0,
                        RefParametr = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId),
                        Name = contentItem.Name,
                        LineCode = contentItem.Code,
                        Total = 0,
                        Period = period
                    };

                    pfhd2017Service.Save(record);
                }
                else
                {
                    // если строчка есть
                    var existRecords = docData.Where(x => x.LineCode.Equals(contentItem.Code) && !x.Name.Equals(contentItem.Name));

                    foreach (var records in existRecords)
                    {
                        var doc = pfhd2017Service.GetItem<F_F_PlanPaymentIndex>(records.ID);
                        doc.Name = contentItem.Name;

                        pfhd2017Service.Save(doc);
                    }
                }
            }
        }

        [Transaction]
        private void ExpensePaymentContent(int docId)
        {
            var docContent = pfhd2017Service.GetItems<T_F_IndicatorsPfhd>().Where(x => x.Detail.Equals(1)).OrderBy(x => x.Code);

            var docData = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId).ExpensePaymentIndex.Select(x => new { x.ID, x.LineCode, x.Name }).ToList();

            foreach (var contentItem in docContent)
            {
                // если нет строчки то добавляем
                if (!docData.Any(x => x.LineCode.Equals(contentItem.Code)))
                {
                    var record = new F_Fin_ExpensePaymentIndex
                    {
                        ID = 0,
                        RefParametr = pfhd2017Service.GetItem<F_F_ParameterDoc>(docId),
                        Name = contentItem.Name,
                        LineCode = contentItem.Code,
                        TotalSumNextYear = 0,
                        TotalSumFirstPlanYear = 0,
                        TotalSumSecondPlanYear = 0,
                        Fz44SumNextYear = 0,
                        Fz44SumFirstPlanYear = 0,
                        Fz44SumSecondPlanYear = 0,
                        Fz223SumNextYear = 0,
                        Fz223SumFirstPlanYear = 0,
                        Fz223SumSecondPlanYear = 0
                    };

                    pfhd2017Service.Save(record);
                }
                else
                {
                    // если строчка есть
                    var existRecords = docData.Where(x => x.LineCode.Equals(contentItem.Code) && !x.Name.Equals(contentItem.Name));

                    foreach (var records in existRecords)
                    {
                        var doc = pfhd2017Service.GetItem<F_Fin_ExpensePaymentIndex>(records.ID);
                        doc.Name = contentItem.Name;

                        pfhd2017Service.Save(doc);
                    }
                }
            }
        }

        private void ExpensePaymentGridSummRows(string[] codes, string lineCodeRow, List<F_Fin_ExpensePaymentIndex> rows)
        {
            var sumRow = rows.FirstOrDefault(x => x.LineCode.Equals(lineCodeRow));

            if (sumRow == null)
            {
                return;
            }

            var sums = rows.Where(x => codes.Contains(x.LineCode)).ToList();

            if (sums.Any())
            {
                sumRow.TotalSumNextYear = sums.Sum(x => x.TotalSumNextYear);
                sumRow.TotalSumFirstPlanYear = sums.Sum(x => x.TotalSumFirstPlanYear);
                sumRow.TotalSumSecondPlanYear = sums.Sum(x => x.TotalSumSecondPlanYear);
                sumRow.Fz44SumNextYear = sums.Sum(x => x.Fz44SumNextYear);
                sumRow.Fz44SumFirstPlanYear = sums.Sum(x => x.Fz44SumFirstPlanYear);
                sumRow.Fz44SumSecondPlanYear = sums.Sum(x => x.Fz44SumSecondPlanYear);
                sumRow.Fz223SumNextYear = sums.Sum(x => x.Fz223SumNextYear);
                sumRow.Fz223SumFirstPlanYear = sums.Sum(x => x.Fz223SumFirstPlanYear);
                sumRow.Fz223SumSecondPlanYear = sums.Sum(x => x.Fz223SumSecondPlanYear);

                pfhd2017Service.Save(sumRow);
            }
        }

        private void ExpensePaymentGridSummRow(F_Fin_ExpensePaymentIndex sumRow)
        {
            if (sumRow == null)
            {
                return;
            }

            sumRow.TotalSumNextYear = sumRow.Fz44SumNextYear + sumRow.Fz223SumNextYear;
            sumRow.TotalSumFirstPlanYear = sumRow.Fz44SumFirstPlanYear + sumRow.Fz223SumFirstPlanYear;
            sumRow.TotalSumSecondPlanYear = sumRow.Fz44SumSecondPlanYear + sumRow.Fz223SumSecondPlanYear;
        }
    }
}
