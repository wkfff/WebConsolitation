using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.ResultsOfActivity;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    ///   Контроллер для представления "Информация о результатах деятельности и использование имущества"
    /// </summary>
    public sealed class ResultsOfActivityViewController : SchemeBoundController
    {
        private readonly IAuthService auth;

        private readonly CashReceiptsViewModel cashReceiptsModel = new CashReceiptsViewModel();

        private readonly FinNFinAssetsViewModel finNFinAssetsModel = new FinNFinAssetsViewModel();

        private readonly MembersOfStaffViewModel membersOfStaffModel = new MembersOfStaffViewModel();

        private readonly PropertyUseViewModel propertyUseModel = new PropertyUseViewModel();
        
        private readonly IResultsOfActivityService service;
        private readonly IChangeLogService logService;

        public ResultsOfActivityViewController()
        {
            service = Resolver.Get<IResultsOfActivityService>();
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        #region MembersOfStaff

        public ActionResult MembersOfStaffLoad(int recId)
        {
            var data = service.GetItems<F_ResultWork_Staff>()
                .Where(p => p.RefParametr.ID == recId)
                .Select(
                    p => new MembersOfStaffViewModel
                        {
                            MembersOfStaffID = p.ID, 
                            BeginYear = p.BeginYear, 
                            EndYear = p.EndYear, 
                            AvgSalary = p.AvgSalary
                        });

            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult MembersOfStaffSave(FormCollection values, int recId)
        {
            var result = new AjaxFormResult();

            try
            {
                var fieldID = values[membersOfStaffModel.NameOf(() => membersOfStaffModel.MembersOfStaffID)];
                var record = new F_ResultWork_Staff();

                if (string.IsNullOrEmpty(fieldID))
                {
                    record.ID = 0;
                    record.TaskID = 0;
                    record.SourceID = 0;
                    record.RefParametr = service.GetItem<F_F_ParameterDoc>(recId);
                }
                else
                {
                    record = service.GetItem<F_ResultWork_Staff>(Convert.ToInt32(fieldID));
                }

                record.EndYear = Convert.ToDecimal(values[membersOfStaffModel.NameOf(() => membersOfStaffModel.EndYear)]);
                record.BeginYear = Convert.ToDecimal(values[membersOfStaffModel.NameOf(() => membersOfStaffModel.BeginYear)]);
                record.AvgSalary = Convert.ToDecimal(values[membersOfStaffModel.NameOf(() => membersOfStaffModel.AvgSalary)]);

                if (record.AvgSalary < 0)
                {
                    throw new InvalidDataException(
                        string.Concat(
                            "Значение поля ",
                            membersOfStaffModel.DescriptionOf(() => membersOfStaffModel.AvgSalary),
                            @" не должно быть отрицательным"));
                }

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

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

        #endregion

        #region FinNFinAssets

        public ActionResult FinNFinAssetsLoad(int recId)
        {
            return new AjaxStoreResult(service.GetFinNFinAssetsItem(recId), 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult FinNFinAssetsSave(FormCollection values, int recId)
        {
            var result = new AjaxFormResult();

            try
            {
                var validationError = FinNFinAssetsValidateData(values);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                decimal? val;
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.AmountOfDamageCompensation)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.AmountOfDamageCompensation)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.AmountOfDamageCompensationID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.AmountOfDamageCompensation), 
                    0);

                int? param;

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotal)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotal)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotalID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ChangingArrearsTotal), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Income)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Income)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Income), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Expenditure)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Expenditure)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.Expenditure), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovableProperty)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovableProperty)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ImmovableProperty), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuableProperty)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuableProperty)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.ParticularlyValuableProperty), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal), 
                    param);

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesRefTypeIxm) + "_SelIndex"] != "-1")
                {
                    param = Convert.ToInt32(
                        values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesRefTypeIxm) + "_Value"]);
                }
                else
                {
                    param = null;
                }

                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayables)] != string.Empty)
                {
                    val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayables)]);
                }
                else
                {
                    val = null;
                }

                service.SaveValRowFinNFinAssets(
                    recId, 
                    values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesID)], 
                    val, 
                    finNFinAssetsModel.DescriptionIdOf(() => finNFinAssetsModel.OverduePayables), 
                    param);

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                logService.WriteChangeDocDetail(service.GetItem<F_F_ParameterDoc>(recId));
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        public ActionResult GetTypeOfChange()
        {
            var data = (from p in service.GetItems<FX_Fin_TypeIzmen>()
                        where p.ID != 0
                        select p).ToList();

            return new AjaxStoreResult(data, data.Count);
        }
        
        #endregion

        #region CashReceipts

        public ActionResult CashReceiptsLoad(int recId)
        {
            var data = service.GetItems<F_ResultWork_CashReceipts>()
                .Where(p => p.RefParametr.ID == recId)
                .Select(
                    p => new CashReceiptsViewModel
                        {
                            CashReceiptsID = p.ID, 
                            PaidServices = p.PaidServices, 
                            TaskGrant = p.TaskGrant, 
                            Total = p.Total, 
                            ActionGrant = p.ActionGrant, 
                            BudgetFunds = p.BudgetFunds
                        });
            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult CashReceiptsSave(FormCollection values, int recId)
        {
            var result = new AjaxFormResult();

            try
            {
                var fieldID = values[cashReceiptsModel.NameOf(() => cashReceiptsModel.CashReceiptsID)];
                var record = new F_ResultWork_CashReceipts
                    {
                        ActionGrant = Convert.ToDecimal(values[cashReceiptsModel.NameOf(() => cashReceiptsModel.ActionGrant)]),
                        BudgetFunds = Convert.ToDecimal(values[cashReceiptsModel.NameOf(() => cashReceiptsModel.BudgetFunds)]),
                        PaidServices = Convert.ToDecimal(values[cashReceiptsModel.NameOf(() => cashReceiptsModel.PaidServices)]),
                        TaskGrant = Convert.ToDecimal(values[cashReceiptsModel.NameOf(() => cashReceiptsModel.TaskGrant)]),
                        Total = Convert.ToDecimal(values[cashReceiptsModel.NameOf(() => cashReceiptsModel.Total)])
                    };

                var validationError = CashReceiptsValidateData(record);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                if (string.IsNullOrEmpty(fieldID))
                {
                    record.ID = 0;
                    record.TaskID = 0;
                    record.SourceID = 0;
                    record.RefParametr = service.GetItem<F_F_ParameterDoc>(recId);
                    service.Save(record);
                }
                else
                {
                    var rec = service.GetItem<F_ResultWork_CashReceipts>(Convert.ToInt32(fieldID));
                    rec.ActionGrant = record.ActionGrant;
                    rec.BudgetFunds = record.BudgetFunds;
                    rec.PaidServices = record.PaidServices;
                    rec.TaskGrant = record.TaskGrant;
                    rec.Total = record.Total;
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                logService.WriteChangeDocDetail(service.GetItem<F_F_ParameterDoc>(recId));
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }
        
        #endregion
        
        public AjaxStoreResult GetKOSGY(int limit, int start, string query)
        {
            var data = service.GetItems<D_KOSGY_KOSGY>()
                .Where(p => (p.Code.Equals("500") || p.Code.Equals("530") || p.Code.StartsWith("2") || p.Code.StartsWith("3")) && (p.Code.Contains(query) || p.Name.Contains(query)))
                .Select(
                    p => new
                    {
                        p.ID,
                        p.Name,
                        p.Code
                    });
            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }
        
        [HttpPost]
        [Transaction]
        public AjaxFormResult ServicesWorksImportFromStateTask(int docId)
        {
            try
            {
                // текущий документ
                var doc = service.GetItem<F_F_ParameterDoc>(docId);

                // одно самое позднее ГЗ для данной организации (при этом года формирования ГЗ и результатов совпадают)
                var gz = service.GetItems<F_F_ParameterDoc>()
                    .Where(x => x.RefUchr.ID == doc.RefUchr.ID && x.RefPartDoc.ID == FX_FX_PartDoc.StateTaskDocTypeID && x.RefYearForm == doc.RefYearForm)
                    .OrderByDescending(x => x.OpeningDate).First();

                // Все услуги для данного ГЗ. (Без повторения)
                var data = service.GetItems<F_F_GosZadanie>()
                    .Where(p => p.RefParametr.ID == gz.ID)
                    .Select(p => new { RefVedPch = p.RefVedPch.ID })
                    .ToList().Distinct();

                var docdata = service.GetItems<F_ResultWork_ShowService>().Where(p => p.RefParametr.ID == docId);
                foreach (var item in docdata)
                {
                    service.Delete<F_ResultWork_ShowService>(item.ID);
                }

                foreach (var record in data.Select(p => new F_ResultWork_ShowService
                    {
                        ID = 0, 
                        RefParametr = doc, 
                        RefVedPch = service.GetItem<D_Services_VedPer>(p.RefVedPch), 
                        NRazdel = 0, 
                        Customers = 0, 
                        Complaints = 0
                    }))
                {
                    service.Save(record);
                }

                logService.WriteChangeDocDetail(doc);

                return new AjaxFormResult
                    {
                        Success = true, 
                        IsUpload = true, 
                        ExtraParams =
                            {
                                new Parameter("msg", "Иморт выполнен успешно")
                            }
                    };
            }
            catch (Exception)
            {
                return new AjaxFormResult
                    {
                        Success = false, 
                        IsUpload = true, 
                        ExtraParams =
                            {
                                new Parameter("responseText", "Государственное задание за отчетный год отсутствует. Услуги не найдены")
                            }
                    };
            }
        }
        
        [HttpPost]
        [Transaction]
        public AjaxFormResult ServicesWorks2016ImportFromStateTask(int docId)
        {
            try
            {
                // текущий документ
                var doc = service.GetItem<F_F_ParameterDoc>(docId);

                // одно самое позднее ГЗ для данной организации (при этом года формирования ГЗ и результатов совпадают)
                var gz = service.GetItems<F_F_ParameterDoc>()
                    .Where(x => x.RefUchr.ID.Equals(doc.RefUchr.ID)
                                && x.RefPartDoc.ID.Equals(FX_FX_PartDoc.StateTaskDocTypeID)
                                && x.RefYearForm.ID.Equals(doc.RefYearForm.ID))
                    .OrderByDescending(x => x.OpeningDate).First();

                // Все услуги для данного ГЗ. (Без повторения)
                var data = gz.StateTasks2016.GroupBy(x => x.RefService.NameCode).Select(group => group.First());

                var docdata = service.GetItems<F_F_ShowService2016>().Where(p => p.RefParametr.ID == docId);
                foreach (var item in docdata)
                {
                    service.Delete<F_F_ShowService2016>(item.ID);
                }

                foreach (var record in data.Select(
                    p => new F_F_ShowService2016
                        {
                            RefParametr = doc,
                            RefService = p.RefService,
                            NRazdel = 0,
                            Customers = 0,
                            Complaints = 0
                        }))
                {
                    service.Save(record);
                }

                logService.WriteChangeDocDetail(doc);

                return new AjaxFormResult
                    {
                        Success = true,
                        IsUpload = true,
                        ExtraParams =
                            {
                                new Parameter("msg", "Иморт выполнен успешно")
                            }
                    };
            }
            catch (Exception)
            {
                return new AjaxFormResult
                    {
                        Success = false,
                        IsUpload = true,
                        ExtraParams =
                            {
                                new Parameter("responseText", "Государственное задание за отчетный год отсутствует. Услуги не найдены")
                            }
                    };
            }
        }

        #region PropertyUse

        public ActionResult PropertyUseLoad(int recId)
        {
            return new AjaxStoreResult(service.GetPropertyUseItem(recId), 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult PropertyUseSave(FormCollection values, int recId)
        {
            var result = new AjaxFormResult();

            try
            {
                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.AmountOfFundsFromDisposalID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.AmountOfFundsFromDisposalBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.AmountOfFundsFromDisposalEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.AmountOfFundsFromDisposalID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.BookValueOfRealEstateTotalID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.BookValueOfRealEstateTotalBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.BookValueOfRealEstateTotalEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.BookValueOfRealEstateTotalID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyGivenOnRentID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyGivenOnRentBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyGivenOnRentEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyGivenOnRentID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyDonatedID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyDonatedBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyDonatedEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.ImmovablePropertyDonatedID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyGivenOnRentID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyGivenOnRentBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyGivenOnRentEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyGivenOnRentID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyDonatedID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyDonatedBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyDonatedEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.MovablePropertyDonatedID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.AreaOfRealEstateTotalID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.AreaOfRealEstateTotalBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.AreaOfRealEstateTotalEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.AreaOfRealEstateTotalID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.GivenOnRentID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.GivenOnRentBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.GivenOnRentEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.GivenOnRentID));

                service.SaveValRowPropertyUse(
                    recId, 
                    values[propertyUseModel.NameOf(() => propertyUseModel.DonatedID)], 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.DonatedBeginYear)]), 
                    Convert.ToDecimal(values[propertyUseModel.NameOf(() => propertyUseModel.DonatedEndYear)]), 
                    propertyUseModel.DescriptionIdOf(() => propertyUseModel.DonatedID));

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                logService.WriteChangeDocDetail(service.GetItem<F_F_ParameterDoc>(recId));
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        #endregion

        #region CopyResultOfWork

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

            var resultWorkStaves = service.GetItems<F_ResultWork_Staff>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();

            var finNFinAssets = service.GetItems<F_ResultWork_FinNFinAssets>()
                .Where(x => x.RefParametr.ID == recId);

            var resultWorkCashRecepients = service.GetItems<F_ResultWork_CashReceipts>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();
            var resultWorkCashPay = service.GetItems<F_ResultWork_CashPay>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();
            var resultWorkShowServices = service.GetItems<F_ResultWork_ShowService>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();

            if (resultWorkStaves.Count == 0 && !finNFinAssets.Any() && resultWorkCashRecepients.Count == 0
                && resultWorkCashPay.Count == 0 && resultWorkShowServices.Count == 0)
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

        [Transaction]
        public RestResult CopyContent(int recId)
        {
            var formData = service.GetItem<F_F_ParameterDoc>(recId);
            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;

            try
            {
                if (service.GetItems<F_ResultWork_Staff>().Any(x => x.RefParametr.ID == idOfLastDoc))
                {
                    var item = service.GetItems<F_ResultWork_Staff>().First(x => x.RefParametr.ID == idOfLastDoc);
                    var resultWorkStaffe = new F_ResultWork_Staff
                        {
                            SourceID = item.SourceID,
                            TaskID = item.TaskID,
                            AvgSalary = item.AvgSalary,
                            BeginYear = item.BeginYear,
                            EndYear = item.EndYear, 
                            RefParametr = formData
                        };
                    service.Save(resultWorkStaffe);
                }

                var finNFinAssets = service.GetItems<F_ResultWork_FinNFinAssets>().Where(x => x.RefParametr.ID == idOfLastDoc);

                if (finNFinAssets.Any())
                {
                    foreach (var resultWorkFinNFinAssetse in finNFinAssets)
                    {
                        service.Save(
                            new F_ResultWork_FinNFinAssets
                                {
                                    SourceID = resultWorkFinNFinAssetse.SourceID,
                                    TaskID = resultWorkFinNFinAssetse.TaskID,
                                    Value = resultWorkFinNFinAssetse.Value,
                                    RefStateValue = resultWorkFinNFinAssetse.RefStateValue,
                                    RefTypeIxm = resultWorkFinNFinAssetse.RefTypeIxm,
                                    RefParametr = formData
                                });
                    }
                }

                if (service.GetItems<F_ResultWork_CashReceipts>().Any(x => x.RefParametr.ID == idOfLastDoc))
                {
                    var item = service.GetItems<F_ResultWork_CashReceipts>().First(x => x.RefParametr.ID == idOfLastDoc);
                    var cashReceipts = new F_ResultWork_CashReceipts
                        {
                            SourceID = item.SourceID,
                            TaskID = item.TaskID,
                            TaskGrant = item.TaskGrant,
                            ActionGrant = item.ActionGrant,
                            BudgetFunds = item.BudgetFunds,
                            PaidServices = item.PaidServices,
                            Total = item.Total,
                            RefParametr = formData
                        };
                    service.Save(cashReceipts);
                }

                var cashPay = service.GetItems<F_ResultWork_CashPay>().Where(x => x.RefParametr.ID == idOfLastDoc);

                if (cashPay.Any())
                {
                    foreach (var p in cashPay)
                    {
                        service.Save(
                            new F_ResultWork_CashPay
                                {
                                    SourceID = p.SourceID,
                                    TaskID = p.TaskID,
                                    Payment = p.Payment,
                                    CelStatya = p.CelStatya,
                                    RefKosgy = p.RefKosgy,
                                    RefRazdPodr = p.RefRazdPodr,
                                    RefVidRash = p.RefVidRash,
                                    RefParametr = formData
                                });
                    }                   
                }

                var showServices = service.GetItems<F_ResultWork_ShowService>().Where(x => x.RefParametr.ID == idOfLastDoc);

                if (showServices.Any())
                {
                    foreach (var resultWorkShowService in showServices)
                    {
                        service.Save(
                            new F_ResultWork_ShowService
                                {
                                    SourceID = resultWorkShowService.SourceID,
                                    TaskID = resultWorkShowService.TaskID,
                                    Customers = resultWorkShowService.Customers,
                                    Complaints = resultWorkShowService.Complaints,
                                    Reaction = resultWorkShowService.Reaction,
                                    RefVedPch = resultWorkShowService.RefVedPch,
                                    RefParametr = formData
                                });
                    }
                }

                var useProperties = service.GetItems<F_ResultWork_UseProperty>().Where(x => x.RefParametr.ID == idOfLastDoc);

                if (useProperties.Any())
                {
                    foreach (var useProperty in useProperties)
                    {
                        service.Save(
                            new F_ResultWork_UseProperty
                                {
                                    SourceID = useProperty.SourceID,
                                    TaskID = useProperty.TaskID,
                                    BeginYear = useProperty.BeginYear,
                                    EndYear = useProperty.EndYear,
                                    RefStateValue = useProperty.RefStateValue,
                                    RefParametr = formData
                                });
                    }
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

        #endregion

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportActivityResultService.Serialize(auth, service.Load<F_F_ParameterDoc>(recId)), 
                "application/xml", 
                string.Concat("activityResult", DateTime.Now.ToString("yyyymmddhhmmss"), ".xml"));
        }

        private string FinNFinAssetsValidateData(FormCollection values)
        {
            const string Msg = "Значение \"{0}\" не может быть меньше ноля!<br>";
            const string Msg1 = "Значение \"{0}\" должно быть заполнено!<br>";

            var message = new StringBuilder(string.Empty);
            decimal val;

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotal)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ChangingArrearsTotal)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotal)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ChangingArrearsTotal)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Income)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.Income)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Income)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.Income)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Expenditure)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.Expenditure)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.Expenditure)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.Expenditure)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.InfAboutCarryingValueTotal)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovableProperty)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ImmovableProperty)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovableProperty)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ImmovableProperty)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuableProperty)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ParticularlyValuableProperty)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuableProperty)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.ParticularlyValuableProperty)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal)));
                }
            }

            if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayables)] == string.Empty)
            {
                if (values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesRefTypeIxm)] != "Без изменений")
                {
                    message.Append(Msg1.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.OverduePayables)));
                }
            }
            else
            {
                val = Convert.ToDecimal(values[finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayables)]);
                if (val < 0)
                {
                    message.Append(Msg.FormatWith(finNFinAssetsModel.DescriptionOf(() => finNFinAssetsModel.OverduePayables)));
                }
            }

            return message.ToString();
        }

        private string CashReceiptsValidateData(F_ResultWork_CashReceipts record)
        {
            const string Msg = "Значение поля \"{0}\" должно быть больше либо равно сумме остальных полей<br>";

            var summ = record.TaskGrant + record.ActionGrant + record.BudgetFunds + record.PaidServices;

            return record.Total < summ ? Msg.FormatWith(cashReceiptsModel.DescriptionOf(() => cashReceiptsModel.Total)) : string.Empty;
        }
    }
}
